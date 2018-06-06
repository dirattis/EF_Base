using PHS.Model;
using PHS.Model.VirtualDataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PHS.Data.Base
{
	public class CommonDB : BaseDB
	{
		#region [ Métodos de Inclusão ]

		/// <summary>Inclui o item no banco</summary>
		/// <returns>Objeto</returns>
		public static void Insert<T>(T item)
			where T : Entity, new()
		{
			//Verifica se existe algum item
			if (item == null)
				return;

			using (DataBaseContext db = GetDatabaseContext())
			{
				//Define os valores padrões do item
				SetDefaultValues(item, Enumerators.DatabaseOperations.Insert);

				db.Set<T>().Add(item);
				db.Entry(item).State = EntityState.Added;

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

		/// <summary>Efetua a inclusão da lista enviada</summary>
		/// <param name="type">lista de itens</param>
		/// <returns>Objeto</returns>
		public static void Insert<T>(List<T> itens)
			where T : Entity, new()
		{
			//Verifica se possui item na lista
			if (itens == null || itens.Count == 0)
				return;

			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> tabela = db.Set<T>();

				itens.ForEach(i =>
				{
					//Define os valores padrões do item
					SetDefaultValues(i, Enumerators.DatabaseOperations.Insert);

					tabela.Add(i);
					db.Entry(i).State = EntityState.Added;
				});

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

		#endregion [ Métodos de Inclusão ]

		#region [ Atualizar ]

		/// <summary>Efetua a atualização do objeto informado</summary>
		/// <param name="cod_anexo">ID do item</param>
		/// <returns>Objeto</returns>
		public static void Update<T>(T item, bool delete = false)
			where T : Entity, new()
		{
			using (DataBaseContext db = GetDatabaseContext())
			{
				//Define os valores padrões do item
				SetDefaultValues(item, Enumerators.DatabaseOperations.Update);

				//Define o estado como modificado
				db.Entry(item).State = EntityState.Modified;

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

		/// <summary>Atualiza a lista de objetos informados</summary>
		/// <typeparam name="T">Tipo de objeto para ser atualizado</typeparam>
		/// <param name="itens">Lista de entidades</param>
		/// <returns></returns>
		public static void Update<T>(List<T> itens, bool delete = false)
			where T : Entity, new()
		{
			//Verifica se possui item na lista
			if (itens == null || itens.Count == 0)
				return;

			using (DataBaseContext db = GetDatabaseContext())
			{
				//Atualiza todos os itens
				itens.ForEach(item =>
				{
					//Define os valores padrões do item
					SetDefaultValues(item, Enumerators.DatabaseOperations.Update);
					// Define o estado como modificado
					db.Entry(item).State = EntityState.Modified;
				});

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

		#endregion [ Atualizar ]

		#region [ Métodos para Excluir o item ]
		/// <summary>Exclui logicamente o objeto solicitado</summary>
		/// <param name="cod_anexo">ID do item</param>
		/// <returns>Objeto</returns>
		public static void Delete<T>(T item)
			where T : Entity, new()
		{
			//Define os valores padrões do item
			SetDefaultValues(item, Enumerators.DatabaseOperations.Delete);

			//Exclusão lógica do item
			Update(item, true);
		}

        /// <summary>Exclui lógicamente a lista de projetos informados</summary>
        /// <param name="itens">Lista de entidades</param>
        /// <returns>Objeto</returns>
        public static void Delete<T>(List<T> itens)
			where T : Entity, new()
		{
			//Verifica se possui item na lista
			if (itens == null || itens.Count == 0)
				return;

			//Define os valores padrões do item
			itens.ForEach(i => SetDefaultValues(i, Enumerators.DatabaseOperations.Delete));

			//Exclusão lógica do item
			Update(itens, true);
		}

		/// <summary>Exclui lógicamente os objectos pela chave</summary>
		/// <param name="cod_anexo">ID do item</param>
		/// <returns>Objeto</returns>
		public static void Delete<T>(T entidade, List<int> keys)
			where T : Entity, new()
		{
			if (keys.Count == 0)
				return;

			//Efetua a exclusão dos itens
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();
				List<T> itens = new List<T>();

				keys.ForEach(i =>
				{
					T item = table.Find(i);
					if (item != null)
						itens.Add(item);
				}
				);

				//Exclui todos os itens
				if (itens.Count > 0)
					Delete(itens);
			}
		}

        /// <summary>Exclui físicamente a lista de objetos</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itens"></param>
		public static void Remove<T>(List<T> itens) where T : Entity, new()
		{
			using (DataBaseContext db = GetDatabaseContext())
			{
				foreach (T item in itens)
				{
					db.Set<T>().Remove(item);
				}

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

        /// <summary>Exclui físicamente o objeto</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
		public static void Remove<T>(T item) where T : Entity, new()
		{
			using (DataBaseContext db = GetDatabaseContext())
			{
				db.Set<T>().Remove(item);

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{
					throw new Exception(FormatDBValidationError(ex));
				}
			}
		}

		#endregion [ Métodos para Excluir o item ]

		#region [ Métodos Obter ]

		/// <summary>Retorna o objeto solicitado</summary>
		/// <param name="cod_anexo">ID do item</param>
		/// <returns>Objeto</returns>
		public static T Get<T>(T item, int key) where T : Entity, new()
		{
			T result = default(T);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();
				result = table.Find(key);
			}

			//Retorna
			return result;
		}

		/// <summary>Retorna o objeto solicitado</summary>
		/// <param name="cod_anexo">ID do item</param>
		/// <returns>Objeto</returns>
		public static T Get<T>(T item, long key) where T : Entity, new()
		{
			T result = default(T);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();
				result = table.Find(key);
			}

			//Retorna
			return result;
		}

		/// <summary>Retorna o objeto solicitado</summary>
		/// <param name="item">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <param name="loadRelatedEntities">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Objeto</returns>
		public static T Get<T>(T item, Expression<Func<T, Boolean>> filter, EntityList<T> loadRelatedEntities) where T : Entity, new()
		{
			T result = default(T);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				var table = db.Set<T>().Where(filter).Where(x => !x.Deleted);

				if (loadRelatedEntities != null && loadRelatedEntities.List != null && loadRelatedEntities.List.Any())
					foreach (var path in loadRelatedEntities.List)
						table = table.Include(path);

				result = table.AsNoTracking().FirstOrDefault();
			}

			//Retorna
			return result;
		}

		/// <summary>Retorna o objeto solicitado</summary>
		/// <param name="entidade">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <param name="carregarEntidades">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Objeto</returns>
		public static T1 Get<T1, T2>(T1 item, Expression<Func<T1, Boolean>> filter, params Expression<Func<T1, T2>>[] loadRelatedEntities)
			where T1 : Entity, new()
		{
			T1 result = default(T1);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				var table = db.Set<T1>().Where(filter).Where(x => !x.Deleted);

				if (loadRelatedEntities != null && loadRelatedEntities.Count() > 0)
					foreach (var path in loadRelatedEntities)
						table = table.Include(path);

				result = table.AsNoTracking().FirstOrDefault();
			}

			//Retorna
			return result;
		}

		#endregion [ Métodos Obter ]

		#region [ Métodos Listar ]

		/// <summary>Retorna a lista de objetos</summary>
		/// <returns>Lista de objetos na base</returns>
		public static List<T> Select<T>(T entity) where T : Entity, new()
		{
			List<T> results = default(List<T>);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();
				results = table.Where(x => !x.Deleted).AsNoTracking().ToList();
			}

			//Retorna
			return results;
		}

		/// <summary>Retorna a lista de objetos</summary>
		/// <returns>Lista de objetos na base</returns>
		public static List<T> Select<T>(T entity, List<Int32> keys) where T : Entity, new()
		{
			List<T> results = new List<T>();

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> tabela = db.Set<T>();

				keys.ForEach(i =>
				    {
					    T item = tabela.Find(i);
					    if (item != null)
						    results.Add(item);
				    }
				);
			}

			//Retorna
			return results.Where(x => !x.Deleted).ToList();
		}

		public static List<T1> Select<T1, T2>(T1 entity, Expression<Func<T1, Boolean>> filter, params Expression<Func<T1, T2>>[] loadRelatedEntities)
				where T1 : Entity, new()
		{
			var results = new List<T1>();

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				IQueryable<T1> table;
				if (filter != null)
					table = db.Set<T1>().Where(filter);
				else
					table = db.Set<T1>().AsQueryable();
				if (loadRelatedEntities != null && loadRelatedEntities.Count() > 0)
					foreach (var path in loadRelatedEntities)
						table = table.Include(path);

				results = table.Where(x => !x.Deleted).AsNoTracking().ToList();
			}

			//Retorna
			return results;
		}

		public static List<T> Select<T>(T entity, Expression<Func<T, Boolean>> filter, EntityList<T> loadRelatedEntities, Int32 page = -1, Int32 pageSize = -1, Boolean asc = true) where T : Entity, new()
		{
			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				IQueryable<T> query = null;

				if (filter == null)
					query = db.Set<T>().AsQueryable();
				else
					query = db.Set<T>().Where(filter);

				//Carrega as entidades solicitadas
				if (loadRelatedEntities != null && loadRelatedEntities.List != null && loadRelatedEntities.List.Any())
					foreach (var path in loadRelatedEntities.List)
						query = query.Include(path);

				//Efetua a busca no banco
				if (page > -1 && pageSize > -1)
					query = query.Skip(page * pageSize).Take(pageSize);

				//Retorna
				return query.Where(x => !x.Deleted).AsNoTracking().ToList();
			}
		}

		/// <summary>Retorna a lista de objetos aplicando um filtro</summary>
		/// <param name="filter">Expressão a ser usada como filtro</param>
		/// <returns>Lista de objetos encontrados</returns>
		public static List<T> Select<T, TKey>(T entity, Expression<Func<T, Boolean>> filter, EntityList<T> loadRelatedEntities, Expression<Func<T, TKey>> ordenacao, Int32 page = -1, Int32 pageSize = -1, Boolean asc = true) where T : Entity, new()
		{
			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				IQueryable<T> query = null;

				if (filter == null)
					query = db.Set<T>().AsQueryable();
				else
					query = db.Set<T>().Where(filter);

				//Carrega as entidades solicitadas
				if (loadRelatedEntities != null && loadRelatedEntities.List != null && loadRelatedEntities.List.Any())
					foreach (var path in loadRelatedEntities.List)
						query = query.Include(path);

				//Ordena os itens
				if (ordenacao != null)
					if (!asc)
						query = query.OrderByDescending(ordenacao);
					else
						query = query.OrderBy(ordenacao);

				//Efetua a busca no banco
				if (page > -1 && pageSize > -1)
					query = query.Skip(page * pageSize).Take(pageSize);

				//Retorna
				return query.Where(x => !x.Deleted).AsNoTracking().ToList();
			}
		}

		/// <summary>
		/// Função para executar a busca com a possibilidade de uma ordenação complexa
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="filter"></param>
		/// <param name="orderBy"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static List<T> GenericSelect<T>(T entity, Func<IQueryable<T>, IQueryable<T>> filter, Func<IQueryable<T>, IQueryable<T>> orderBy, Int32 page = -1, Int32 pageSize = -1) where T : Entity, new()
		{
			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				IQueryable<T> query = null;

				query = db.Set<T>().AsQueryable();

				//filtro dos itens
				if (filter != null)
					query = filter(query);

				//Ordena os itens
				if (orderBy != null)
					query = orderBy(query);

				//Efetua a busca no banco
				if (page > -1 && pageSize > -1)
					query = query.Skip(page * pageSize).Take(pageSize);

				//Retorna
				return query.Where(x => !x.Deleted).AsNoTracking().ToList();
			}
		}

		public static List<T> GenericSelect<T, T2>(T entity,
			Func<IQueryable<T>, IQueryable<T>> filter, Func<IQueryable<T>, IQueryable<T>> orderBy,
			Int32 page = -1, Int32 pageSize = -1,
			params Expression<Func<T, T2>>[] loadRelatedEntities) where T : Entity, new()
		{
			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				IQueryable<T> query = null;

				query = db.Set<T>().AsQueryable();

				if (loadRelatedEntities != null && loadRelatedEntities.Count() > 0)
					foreach (var path in loadRelatedEntities)
						query = query.Include(path);

				//filtro dos itens
				if (filter != null)
					query = filter(query);

				//Ordena os itens
				if (orderBy != null)
					query = orderBy(query);

				//Efetua a busca no banco
				if (page > -1 && pageSize > -1)
					query = query.Skip(page * pageSize).Take(pageSize);

				//Retorna
				return query.AsNoTracking().ToList();
			}
		}

		/// <summary>Retorna a quantidade de objetos encontrados com o filtro informado</summary>
		/// <param name="filter">Expressão a ser usada como filtro</param>
		/// <returns>Quantidade de objetos encontrados</returns>
		public static Int32 SelectCount<T>(T entity, Expression<Func<T, Boolean>> filter) where T : Entity, new()
		{
			Int32 count = 0;

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();

				//Efetua a busca dos itens
				if (filter != null)
					count = table.Where(filter).Where(x => !x.Deleted).Count();
				else
					count = table.Where(x => !x.Deleted).Count();
			}

			return count;
		}

		public static Int32 GenericSelectCount<T>(T entity, Func<IQueryable<T>, IQueryable<T>> filter) where T : Entity, new()
		{
			Int32 count = 0;

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				DbSet<T> table = db.Set<T>();

				//Efetua a busca dos itens
				if (filter != null)
					count = filter(table.AsQueryable()).Where(x => !x.Deleted).Count();
				else
					count = table.Where(x => !x.Deleted).Count();
			}

			return count;
		}

		/// <summary>Retorna a lista de objetos</summary>
		/// <returns>Lista de objetos na base</returns>
		public static List<T> ExecuteQuery<T>(string rawSQL, params object[] parameters) where T : Entity, new()
		{
			List<T> result = default(List<T>);

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
				result = db.Database.SqlQuery<T>(rawSQL, parameters).ToList();

			//Retorna
			return result;
		}

        public static List<T> ExecuteQuery<T>(string rawSQL) where T : Entity, new()
        {
            List<T> result = default(List<T>);

            //Busca o objeto solicitado
            using (DataBaseContext db = GetDatabaseContext())
                result = db.Database.SqlQuery<T>(rawSQL).ToList();

            //Retorna
            return result;
        }

        /// <summary>Retorna a lista de objetos</summary>
        /// <returns>Lista de objetos na base</returns>
        public static int ExecuteCount<T>(string rawSQLCount, params object[] parameters) where T : Entity, new()
		{
			int count = 0;

			//Busca o objeto solicitado
			using (DataBaseContext db = GetDatabaseContext())
			{
				count = db.Database.SqlQuery<int>(rawSQLCount, parameters).FirstOrDefault();
			}

			//Retorna
			return count;
		}

        public static List<TResult> QueryVirtualResult<TSource, TResult>(
                                              Expression<Func<TSource, TResult>> selector,
                                              Func<IQueryable<TSource>, IQueryable<TSource>> filter,
                                              Func<IQueryable<TSource>, IQueryable<TSource>> orderBy,
                                              Int32 page = -1,
                                              Int32 pageSize = -1
                                              )
             where TSource : Entity, new()
             where TResult : VirtualCRUDBase, new()
        {

            using (DataBaseContext db = GetDatabaseContext())
            {
                IQueryable<TSource> query = null;

                query = db.Set<TSource>().AsQueryable().Where(x => !x.Deleted);

                //filtro dos itens
                if (filter != null)
                    query = filter(query);

                //Ordena os itens
                if (orderBy != null)
                    query = orderBy(query);

                //Efetua a busca no banco
                if (page > -1 && pageSize > -1)
                    query = query.Skip(page * pageSize).Take(pageSize);

                //Retorna
                return query.Select(selector).ToList();
            }
        }



        public static List<TResult> QueryVirtualResult<TSource, TResult>(
                                                        Expression<Func<TSource, TResult>> selector,
                                                        Expression<Func<TSource, Boolean>> filter)
                                                        where TSource : Entity, new()
                                                        where TResult : VirtualCRUDBase, new()
        {
            var results = new List<TResult>();

            //Busca o objeto solicitado
            using (DataBaseContext db = GetDatabaseContext())
            {
                IQueryable<TSource> query;

                query = db.Set<TSource>().AsQueryable().Where(x => !x.Deleted);

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var a = query.Select(selector);

                results = a.ToList();
            }

            //Retorna
            return results;
        }


        public static List<TResult> QueryWithInclude<TResult>(
                                                Expression<Func<TResult, Boolean>> filter,
                                                List<string> includes)
                                                where TResult : Entity, new()
        {
            var results = new List<TResult>();

            //Busca o objeto solicitado
            using (DataBaseContext db = GetDatabaseContext())
            {
                IQueryable<TResult> query;

                query = db.Set<TResult>().AsQueryable().Where(x => !x.Deleted);

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (includes != null && includes.Any())
                {
                    foreach (var includePath in includes)
                        query = query.Include(includePath);
                }

                results = query.ToList();
            }

            //Retorna
            return results;
        }
        #endregion [ Métodos Listar ]

        #region Exception

        /// <summary>
        /// Tratamento para o caso de falha na inserção de log, coloca o erro em disco e no event viewer
        /// </summary>
        /// <param name="exLog"></param>
        /// <param name="entity"></param>
        public static void LogException(Exception exLog, Log entity = null)
		{
			//Monta a mensagem para gravação
			StringBuilder mensagemErro = new StringBuilder();
			mensagemErro.AppendLine("-------------------------------------------------------------------------------------------------------");
			mensagemErro.AppendFormat("Date: {0}{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Environment.NewLine);
			mensagemErro.AppendLine("Erro ao executar operação de inclusão de erro na base de dados:");
			mensagemErro.AppendFormat("Message: {0}{1}", exLog.Message, Environment.NewLine);
			mensagemErro.AppendFormat("StackTrace: {0}{1}", exLog.StackTrace, Environment.NewLine);
			mensagemErro.AppendLine();
			mensagemErro.Append("------------------------------------------------------------------------");
			mensagemErro.AppendLine();

			if (entity != null)
			{
				mensagemErro.AppendLine("Original: ");
				mensagemErro.AppendFormat("Level: {0}{1}", entity.Level, Environment.NewLine);
				mensagemErro.AppendFormat("CreatedBy: {0}{1}", entity.CreatedBy, Environment.NewLine);
				mensagemErro.AppendFormat("Uri: {0}{1}", entity.Uri, Environment.NewLine);
				mensagemErro.AppendFormat("SourceProcess: {0}{1}", entity.Source, Environment.NewLine);
				mensagemErro.AppendFormat("Message: {0}{1}", entity.Message, Environment.NewLine);
				mensagemErro.AppendFormat("Detail: {0}{1}", entity.Detail, Environment.NewLine);

				mensagemErro.AppendLine("-------------------------------------------------------------------------------------------------------");
				mensagemErro.AppendLine();
			}

			try // Adicionar o log na pasta do UI
			{
				DateTime today = DateTime.Now;
				String[] folderPath = new String[5] { System.AppDomain.CurrentDomain.BaseDirectory, "LOG", today.ToString("yyyy"), today.ToString("MM"), today.ToString("dd") };
				String targetFolder = System.IO.Path.Combine(folderPath);

				Directory.CreateDirectory(targetFolder);

				//Grava o erro em um arquivo .txt
				using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(targetFolder, "Log.txt"), true))
				{
					sw.Write(mensagemErro.ToString());
				}
			}
			catch { }

			try // Adiciona o Log do EventViewe
			{
				//Efetua a gravação do erro no EventViewer
				if (!EventLog.SourceExists("Housekeeping"))
					EventLog.CreateEventSource("Housekeeping", "Application");
				EventLog.WriteEntry("Housekeeping", mensagemErro.ToString(), EventLogEntryType.Error);
			}
			catch { }
		}

		#endregion Exception
	}
}