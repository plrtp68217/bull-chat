import * as signalR from "@microsoft/signalr";

const basePath = import.meta.env.VITE_BASE_PATH || '';

export const createHubConnection = (): signalR.HubConnection => {
  const token = localStorage.getItem('JWT_TOKEN');

  const options: signalR.IHttpConnectionOptions = {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
  };

  if (token) {
    options.accessTokenFactory = () => token;
  }

  return new signalR.HubConnectionBuilder()
    .withUrl(`${basePath}/chatHub`, options)
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        return Math.min(retryContext.elapsedMilliseconds * 2, 10000);
      },
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
};
