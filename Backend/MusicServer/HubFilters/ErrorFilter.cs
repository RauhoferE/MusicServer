using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Serilog;
using System.Net;
using System;

namespace MusicServer.HubFilters
{
    public class ErrorFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(
    HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            Log.Debug($"Calling hub method '{invocationContext.HubMethodName}'");
            try
            {
                return await next(invocationContext);
            }
            //catch (DataNotFoundException)
            //{
            //    throw;
            //}
            //catch (PlaylistNotFoundException)
            //{
            //    await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", "Playlist was not found");
            //    throw;
            //}
            //catch (UserNotFoundException)
            //{
            //    await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", "User was not found");
            //    throw;
            //}
            //catch (SongNotFoundException)
            //{
            //    await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", "Song was not found");
            //    throw;
            //}
            //catch (AlbumNotFoundException)
            //{
            //    await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", "Album was not found");
            //    throw;
            //}
            catch (HubException ex)
            {
                await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Debug($"Exception calling '{invocationContext.HubMethodName}': {ex}");
                await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveErrorMessage", "An unexpted error occured!");
                throw;
            }
        }
    }
}
