import * as signalR from "@microsoft/signalr";

const basePath = import.meta.env.VITE_BASE_PATH || '';

export const createHubConnection = (): signalR.HubConnection => {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${basePath}/chatHub`, {
      skipNegotiation: true,
      transport: signalR.HttpTransportType.WebSockets,
    })
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        return Math.min(retryContext.elapsedMilliseconds * 2, 10000);
      },
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
};
