using PHS.Data.Base;
using PHS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace PHS.Business
{
    public static class BusinessCommon
	{
		#region [ Inserir ]

		/// <summary>Efetua a inclusão do objeto</summary>
		/// <param name="value">Objeto a ser incluído</param>
		public static void InsertAsync<T>(this T item)
			where T : Entity, IEntityInsert, new()
		{
			new Thread(() =>
			{
				try
				{
					CommonDB.Insert<T>(item);
				}
				catch (Exception ex)
				{
					CommonDB.LogException(ex);
				}
			}).Start();
		}

		/// <summary>Efetua a inclusão do objeto</summary>
		/// <param name="value">Objeto a ser incluído</param>
		public static void Insert<T>(this T item)
			where T : Entity, IEntityInsert, new()
		{
			CommonDB.Insert<T>(item);
		}

		/// <summary>Efetua a inclusão do objeto</summary>
		/// <param name="itens">Lista de itens para serem incluídos</param>
		public static void Insert<T>(this List<T> itens)
			where T : Entity, IEntityInsert, new()
		{
			if (itens == null || itens.Count == 0)
				return;

			CommonDB.Insert<T>(itens);
		}

		#endregion [ Inserir ]

		#region [ Atualizar ]

		/// <summary>Atualiza o item informado</summary>
		/// <param name="item">Item</param>
		public static void Update<T>(this T item)
			where T : Entity, IEntityUpdate, new()
		{
			CommonDB.Update<T>(item);
		}

		/// <summary>Inclui / Atualiza os itens da lista</summary>
		/// <param name="itens">Itens a serem Incluídos/Atualizados</param>
		public static void Update<T>(this List<T> itens)
			where T : Entity, IEntityUpdate, new()
		{
			if (itens == null || itens.Count == 0)
				return;

			CommonDB.Update<T>(itens);
		}

		#endregion [ Atualizar ]

		#region [ Métodos para Excluir o item ]

		/// <summary>Exclui logicamente o item</summary>
		/// <param name="item">Item</param>
		/// <returns>Se o item foi excluído com sucesso</returns>
		public static void Delete<T>(this T item)
			where T : Entity, IEntityDelete, new()
		{
			CommonDB.Delete<T>(item);
		}

        /// <summary>Exclui logicamente a lista de itens</summary>
        /// <param name="itens">Lista de itens</param>
        public static void Delete<T>(this List<T> itens)
			where T : Entity, IEntityDelete, new()
		{
			if (itens == null || itens.Count == 0)
				return;

			CommonDB.Delete<T>(itens);
		}

        /// <summary>Exclui logicamente a lista de itens informados</summary>
        /// <param name="entity">Entidade que será excluída</param>
        /// <param name="key">Chaves a serem excluídas</param>
        public static void Delete<T>(this T entity, List<int> key)
			where T : Entity, IEntityDelete, new()
		{
			CommonDB.Delete<T>(entity, key);
		}

		#endregion [ Métodos para Excluir o item ]

		#region [ Métodos Obter ]

		/// <summary>Retorna o item pela chave</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="key">Chave do item para ser retornado</param>
		/// <returns>Item</returns>
		public static T Get<T>(this T entity, Int32 key)
			where T : Entity, IEntityGet, new()
		{
			return CommonDB.Get<T>(entity, key);
		}

		/// <summary>Retorna o item pela chave</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="key">Chave do item para ser retornado</param>
		/// <returns>Item</returns>
		public static T Get<T>(this T entity, long key)
			where T : Entity, IEntityGet, new()
		{
			return CommonDB.Get<T>(entity, key);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <returns>Item</returns>
		public static T Get<T>(this T entity, Expression<Func<T, Boolean>> filter)
			where T : Entity, IEntityGet, new()
		{
			return CommonDB.Get<T>(entity, filter, null);
		}

		#region [ Exclusivos do DB ]

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <param name="loadEntities">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Item</returns>
		public static T Get<T>(this T entity, Expression<Func<T, Boolean>> filter, EntityList<T> loadEntities)
			where T : EntityDB, IEntityGet, new()
		{
			return CommonDB.Get<T>(entity, filter, loadEntities);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <param name="carregarEntidades">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Item</returns>
		public static T1 Get<T1, T2>(this T1 entity, Expression<Func<T1, Boolean>> filter, params Expression<Func<T1, T2>>[] loadEntities)
			where T1 : EntityCRUD, IEntityGet, new()
		{
			return CommonDB.Get<T1, T2>(entity, filter, loadEntities);
		}

		#endregion [ Exclusivos do DB ]

		#endregion [ Métodos Obter ]

		#region [ Métodos Consultar ]

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T>(this T entity)
			where T : Entity, IEntitySelect, new()
		{
			return CommonDB.Select<T>(entity);
		}

		/// <summary>Retorna o item pelo filtro</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <param name="carregarEntidades">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Item</returns>
		public static List<T1> QueryWithEntities<T1, T2>(this T1 entity, Expression<Func<T1, Boolean>> filter, params Expression<Func<T1, T2>>[] loadEntities)
			where T1 : EntityCRUD, IEntityGet, new()
		{
			return CommonDB.Select<T1, T2>(entity, filter, loadEntities);
		}

		/// <summary>Retorna a lista de items de acordo com as chaves</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="key">Chaves para pesquisa</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T>(this T entity, List<Int32> key)
			where T : Entity, IEntitySelect, new()
		{
			return CommonDB.Select<T>(entity, key);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T>(this T entity, Expression<Func<T, Boolean>> filter)
			where T : Entity, IEntitySelect, new()
		{
			return CommonDB.Select<T>(entity, filter, null);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static int QueryCount<T>(this T entity)
			where T : Entity, IEntitySelect, new()
		{
			return CommonDB.SelectCount<T>(entity, null);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro para consulta</param>
		/// <param name="loadEntities">Entidades que devem ser carregadas na consulta</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T>(this T entity, Expression<Func<T, Boolean>> filter, EntityList<T> loadEntities)
			where T : EntityDB, IEntitySelect, new()
		{
			return CommonDB.Select<T>(entity, filter, loadEntities);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro para consulta</param>
		/// <param name="carregarEntidades">Entidades que devem ser carregadas na consulta</param>
		/// <param name="orderBy">Ordem que deve ser retornada</param>
		/// <param name="page">Página atual</param>
		/// <param name="pageSize">Itens por página</param>
		/// <param name="asc">Ordem ascendete/descendente</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T, Tkey>(this T entity,
											Expression<Func<T, Boolean>> filter,
											Expression<Func<T, Tkey>> orderBy,
											Int32 page = -1,
											Int32 pageSize = -1,
											Boolean asc = true)
			where T : EntityDB, IEntitySelect, new()
		{
			return CommonDB.Select<T, Tkey>(entity, filter, null, orderBy, page, pageSize, asc);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro para consulta</param>
		/// <param name="loadEntities">Entidades que devem ser carregadas na consulta</param>
		/// <param name="orderBy">Ordem que deve ser retornada</param>
		/// <param name="page">Página atual</param>
		/// <param name="pageSize">Itens por página</param>
		/// <param name="asc">Ordem ascendete/descendente</param>
		/// <returns>Itens</returns>
		public static List<T> Query<T, Tkey>(this T entity,
											Expression<Func<T, Boolean>> filter,
											EntityList<T> loadEntities,
											Expression<Func<T, Tkey>> orderBy,
											Int32 page = -1,
											Int32 pageSize = -1,
											Boolean asc = true)
			where T : EntityDB, IEntitySelect, new()
		{
			return CommonDB.Select<T, Tkey>(entity, filter, loadEntities, orderBy, page, pageSize, asc);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filter">Filtro</param>
		/// <returns>Itens</returns>
		public static int QueryCount<T>(this T entity, Expression<Func<T, Boolean>> filter)
			where T : EntityDB, new()
		{
			return CommonDB.SelectCount<T>(entity, filter);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro para consulta</param>
		/// <param name="orderBy">Função para ordenação</param>
		/// <param name="page">Página atual</param>
		/// <param name="pageSize">Itens por página</param>
		/// <returns></returns>
		public static List<T> GenericQuery<T>(this T entity,
														 Func<IQueryable<T>, IQueryable<T>> filter,
														 Func<IQueryable<T>, IQueryable<T>> orderBy,
														 Int32 page = -1,
														 Int32 pageSize = -1)
		 where T : Entity, IEntitySelect, new()
		{
			return CommonDB.GenericSelect<T>(entity, filter, orderBy, page, pageSize);
		}

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro para consulta</param>
		/// <param name="orderBy">Função para ordenação</param>
		/// <param name="page">Página atual</param>
		/// <param name="pageSize">Itens por página</param>
		/// <param name="loadEntities">load entities</param>
		/// <returns></returns>
		public static List<T1> GenericQueryWithEntities<T1, T2>(this T1 entity,
														 Func<IQueryable<T1>, IQueryable<T1>> filter,
														 Func<IQueryable<T1>, IQueryable<T1>> orderBy,
														 Int32 page = -1,
														 Int32 pageSize = -1,
														 params Expression<Func<T1, T2>>[] loadEntities)
		 where T1 : Entity, IEntitySelect, new()
		{
			return CommonDB.GenericSelect<T1, T2>(entity, filter, orderBy, page, pageSize, loadEntities);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="entity">Entidade a ser retornada</param>
		/// <param name="filtro">Filtro</param>
		/// <returns>Itens</returns>
		public static int GenericQueryCount<T>(this T entity, Func<IQueryable<T>, IQueryable<T>> filter)
			where T : Entity, new()
		{
			return CommonDB.GenericSelectCount<T>(entity, filter);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="rawSQL">Select para se executado</param>
		/// <param name="parameters">parametros</param>
		/// <returns>Itens</returns>
		public static List<T> ExecuteQuery<T>(string rawSQL, params object[] parameters)
			where T : Entity, IEntityVirtual, new()
		{
			return CommonDB.ExecuteQuery<T>(rawSQL, parameters);
		}

		/// <summary>Retorna a lista de items</summary>
		/// <param name="rawSQL">Select count para se executado</param>
		/// <param name="parameters">parametros</param>
		/// <returns>Itens</returns>
		public static int ExecutarCount<T>(string rawSQL, params object[] parameters)
			where T : Entity, IEntityVirtual, new()
		{
			return CommonDB.ExecuteCount<T>(rawSQL, parameters);
		}

		#endregion [ Métodos Consultar ]

		#region [ Métodos internos ]

		/// <summary>Verifica se o tipo da entidade é Log</summary>
		/// <param name="item">Tipo da entidade</param>
		/// <returns></returns>
		private static bool IsEntityLog(Type item)
		{
			return item.IsSubclassOf(typeof(EntityLog)) || item == typeof(EntityLog);
		}

		private static bool IsEntityDBView(Type item)
		{
			return item.IsSubclassOf(typeof(EntityDBView)) || item == typeof(EntityDBView);
		}

		#endregion [ Métodos internos ]
	}
}