import * as signalR from '@microsoft/signalr';

export const createHubConnection = (token: string): signalR.HubConnection  => {
  return new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5081/chatHub', {
      accessTokenFactory: () => token,
      skipNegotiation: true,
      transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        return Math.min(retryContext.elapsedMilliseconds * 2, 10000);
      }
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
};