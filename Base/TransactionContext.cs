using PHS.Data.Base;
using System;

namespace PHS.Data
{
    public sealed class TransactionContext : IDisposable
    {
        [ThreadStatic]
        public static DataBaseContext StaticDataContext = null;

        /// <summary>
        /// Define se a instância já foi destruida.
        /// </summary>
        private Boolean disposed;

        /// <summary>Inicia o contexto transacional da aplicação (Só pode ser iniciado através do PortalWeb)</summary>
        public TransactionContext()
        {
            StaticDataContext = BaseDB.BeginDBTransaction();
        }

        /// <summary>
        /// Executa a confirmação de mudancas.
        /// </summary>
        public void CommitChanges()
        {
            // Realizando o commit da transação
            if (StaticDataContext == null)
                return;

            if (StaticDataContext.Database.CurrentTransaction != null)
                StaticDataContext.Database.CurrentTransaction.Commit();

            StaticDataContext.PerformingTransaction = false;
            StaticDataContext.Dispose();
            StaticDataContext = null;
        }

        /// <summary>
        /// Executa o cancelamento de mudancas.
        /// </summary>
        public void RollBackChanges()
        {
            // Realizando o rollback da transação
            if (StaticDataContext == null)
                return;

            if (StaticDataContext.Database.CurrentTransaction != null)
                StaticDataContext.Database.CurrentTransaction.Rollback();

            StaticDataContext.PerformingTransaction = false;
            StaticDataContext.Dispose();
            StaticDataContext = null;
        }

        /// <summary>
        /// Implementa o método de dstruição do objeto de banco de dados do Entity Framework.
        /// </summary>
        private void DestroyDataObject()
        {
            if (StaticDataContext != null)
            {
                try
                {
                    if (StaticDataContext.Database.CurrentTransaction != null)
                        StaticDataContext.Database.CurrentTransaction.Rollback();
                }
                // Ocultando erros para casos onde a transação já sofreu rollback.
                catch { }

                StaticDataContext.PerformingTransaction = false;
                StaticDataContext.Dispose();
                StaticDataContext = null;
            }
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        ~TransactionContext()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Realiza a destruição da instância.
        /// </summary>
        /// <param name="disposing">Define se a destruição foi realizada explicitamente.</param>
        internal void Dispose(bool disposing)
        {
            // Verificando se o objeto já está em processo de liberação
            if (this.disposed)
                return;
            disposed = true;

            // Realizando a liberação do objeto e da sessão de banco de dados
            DestroyDataObject();
        }
    }
}
