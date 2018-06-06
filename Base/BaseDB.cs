using PHS.Infrastructure.Utility;
using PHS.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Text;

namespace PHS.Data.Base
{
    /// <summary>Define o a operação realizada no banco.</summary>

    /// <summary>Contexto do banco de dados</summary>
    public class DataBaseContext : AppContext
    {
        public bool PerformingTransaction { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (!PerformingTransaction)
                base.Dispose(disposing);
        }

        public DataBaseContext() { }

        public DataBaseContext(String connectionString) : base(connectionString)
        {
        }
    }

    public class BaseDB
    {
        /// <summary>Utilizado para execuções via PowerShell</summary>
        private static string customConnectionString = null;
        
        /// <summary>Define a conexão (usado para execução via powershell)</summary>
        /// <param name="connectionString"></param>
        public static void SetCustomConnection(string connectionString)
        {
            //DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
            customConnectionString = connectionString;
        }

        /// <summary>Retorna o contexto do banco de dados</summary>
        /// <param name="usarTransacaoContexto"></param>
        /// <returns></returns>
        public static DataBaseContext GetDatabaseContext(bool useTransactionContext = true)
        {
            if (useTransactionContext && TransactionContext.StaticDataContext != null)
                return TransactionContext.StaticDataContext;
            
            if (String.IsNullOrWhiteSpace(customConnectionString))
                return new DataBaseContext();
            else
                return new DataBaseContext(customConnectionString);
        }

        /// <summary>
        /// Inicia uma transação no banco de dados
        /// </summary>
        /// <returns></returns>
        public static DataBaseContext BeginDBTransaction()
        {
            //Busca o contexto do banco
            var db = GetDatabaseContext(false);

            //Inicia a transação
            db.Database.BeginTransaction();
            db.PerformingTransaction = true;

            return db;
        }


        /// <summary>
        /// Define as propriedades básicas do objeto na operação
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="item">Objeto</param>
        /// <param name="operacao">Operação realizada</param>
        public static void SetDefaultValues<T>(T item, Enumerators.DatabaseOperations operation)
        {
            if (item == null)
                return;

            String userName = System.Threading.Thread.CurrentPrincipal.Identity != null && !String.IsNullOrWhiteSpace(System.Threading.Thread.CurrentPrincipal.Identity.Name) ?
                                  System.Threading.Thread.CurrentPrincipal.Identity.Name :
                                  String.Concat(Environment.UserDomainName, @"\", Environment.UserName);

            userName = userName.RemoveClaims();

            switch (operation)
            {
                case Enumerators.DatabaseOperations.Insert:
                    Reflection.SetPropertyValue(item, "Deleted", false);
                    Reflection.SetPropertyValue(item, "CreatedBy", userName);
                    Reflection.SetPropertyValue(item, "CreatedDate", DateTime.Now);
                    break;
                case Enumerators.DatabaseOperations.Update:
                    //Define os valores padrões
                    Reflection.SetPropertyValue(item, "ModifiedBy", userName);
                    Reflection.SetPropertyValue(item, "ModifiedDate", DateTime.Now);
                    break;
                case Enumerators.DatabaseOperations.Delete:
                    //Define os valores padrões
                    Reflection.SetPropertyValue(item, "ModifiedBy", userName);
                    Reflection.SetPropertyValue(item, "ModifiedDate", DateTime.Now);
                    Reflection.SetPropertyValue(item, "Deleted", true);
                    break;
            }
        }

        /// <summary>Busca os erros de validação encontrados</summary>
        /// <param name="erro">Erro</param>
        /// <returns>Mensagem de erro</returns>
        public static string FormatDBValidationError(DbEntityValidationException erro)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in erro.EntityValidationErrors)
            {
                sb.AppendLine(String.Format("A entidade \"{0}\" no estado \"{1}\" teve os seguintes erros de validação:",
                                            item.Entry.Entity.GetType().Name, item.Entry.State));

                foreach (var ve in item.ValidationErrors)
                    sb.AppendLine(String.Format("- Propriedade: \"{0}\", Erro: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
            }
            return sb.ToString();
        }

        /// <summary>Cria o command para execução no banco</summary>
        /// <param name="contextoDB">Contexto do Banco</param>
        /// <param name="procedure">Procedure</param>
        /// <returns>Command</returns>
        public static DbCommand CreateCommand(DataBaseContext contextDB, String procedure)
        {
            DbCommand command = contextDB.Database.Connection.CreateCommand();

            //Define a procedure
            command.CommandText = procedure;
            command.CommandType = System.Data.CommandType.StoredProcedure;

            //Abre a conexão se necessário
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();

            return command;
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        public static void ExecuteProcedure(String procedure, Int32? executionTime, object[] parameters)
        {
            using (DataBaseContext db = GetDatabaseContext())
            {
                if (executionTime.HasValue)
                    db.Database.CommandTimeout = executionTime;

                //Executa a procedure spLimparLog
                if (parameters == null || parameters.Length == 0)
                    db.Database.ExecuteSqlCommand(procedure);
                else
                    db.Database.ExecuteSqlCommand(procedure, parameters);
            }
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        public static void ExecuteProcedure(String procedure, object[] parameters)
        {
            ExecuteProcedure(procedure, null, parameters);
        }

        /// <summary>Executa a procedure informada</summary>
        /// <typeparam name="T">Tipo de objeto retornado</typeparam>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="procedure">Procedure</param>
        /// <param name="converter">Função de conversão de objeto</param>
        /// <returns>Lista do objeto</returns>
        public static List<T> ExecuteProcedure<T>(String procedure, Func<DbDataReader, T> converter, List<DbParameter> parameters = null)
            where T : class, new()
        {
            //Lista de e-mails de tarefas escalonadas para envio
            List<T> itens = new List<T>();

            using (DataBaseContext db = GetDatabaseContext())
            using (DbCommand command = CreateCommand(db, procedure))
            {
                //Define os parâmetros de execução
                if (parameters != null)
                    foreach (var parametro in parameters)
                        command.Parameters.Add(parametro);

                //Executa o comando
                using (DbDataReader dr = command.ExecuteReader())
                    while (dr.Read())
                        itens.Add(converter(dr));
            }

            return itens;
        }
    }
}
