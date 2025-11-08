using Inspection.Application.Common.Interfaces.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>
       : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
           TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var response = await next();
                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            //catch (Exception exp)
            //{
            //    await _unitOfWork.RollBackAsync();
            //    throw;
            //}
            catch {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

       
    }
}
