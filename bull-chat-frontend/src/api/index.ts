import authorizationApi from "./authorization.api";
import messagesApi from "./messages.api"

export const api = {
  auth: authorizationApi,
  messages: messagesApi,
}