﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly IMockProviderRequestHandler _requestHandler;
        private readonly IMockProviderAdminRequestHandler _adminRequestHandler;

        public MockProviderNancyRequestDispatcher(
            IMockProviderRequestHandler requestHandler,
            IMockProviderAdminRequestHandler adminRequestHandler)
        {
            _requestHandler = requestHandler;
            _adminRequestHandler = adminRequestHandler;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetException(new OperationCanceledException());
                return tcs.Task;
            }

            if (context == null)
            {
                tcs.SetException(new ArgumentException("context is null"));
                return tcs.Task;
            }

            Response response;

            try
            {
                response = IsAdminRequest(context.Request) ?
                    _adminRequestHandler.Handle(context) :
                    _requestHandler.Handle(context);
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.Message
                    .Replace(@"\", "\\")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t");

                response = new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = exceptionMessage,
                    Contents = s =>
                    {
                        var bytes = Encoding.UTF8.GetBytes(exceptionMessage);
                        s.Write(bytes, 0, bytes.Length);
                        s.Flush();
                    }
                };
            }

            context.Response = response;
            tcs.SetResult(context.Response);

            return tcs.Task;
        }

        private static bool IsAdminRequest(Request request)
        {
            return request.Headers != null &&
                   request.Headers.Any(x => x.Key == Constants.AdministrativeRequestHeaderKey);
        }
    }
}