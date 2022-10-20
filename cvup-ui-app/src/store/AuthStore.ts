import { UserRegistrationModel } from "../models/Auth";
import { RootStore } from "./RootStore";

export class AuthStore {
  userName = "Tamir";

  constructor(rootStore: RootStore) {}

  registerUser(registrationInfo: UserRegistrationModel) {}
}
