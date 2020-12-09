using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Dapper;
using Dommel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Transactions;

namespace Amphibian.Patrol.Api.Infrastructure
{
    public class DbUnitOfWork:IUnitOfWork
    {
        private DbConnection _connection;
        private TransactionScope _transaction;
        private bool _opened;

        public DbUnitOfWork(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task Begin()
        {
            if (_transaction == null)
            {
                _transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 10, 0),TransactionScopeAsyncFlowOption.Enabled);

                if(_connection.State!=ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                    _opened = true;
                }
                else
                {
                    _opened = false;
                }
                
                //_transaction = await _connection.BeginTransactionAsync().ConfigureAwait(false);
                _connection.EnlistTransaction(System.Transactions.Transaction.Current);
            }
            else
            {
                throw new InvalidOperationException("Transaction already in progress");
            }
        }

        public async Task Commit()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No Transaction in progress");
            }
            else
            {
                _transaction.Complete();
                //await _transaction.CommitAsync().ConfigureAwait(false);
                _transaction.Dispose();
                _transaction = null;
                if(_opened && _connection.State== ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task Rollback()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No Transaction in progress");
            }
            else
            {
                //await _transaction.RollbackAsync().ConfigureAwait(false);
                _transaction.Dispose();
                _transaction = null;
                if (_opened && _connection.State == ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }
    }
}
