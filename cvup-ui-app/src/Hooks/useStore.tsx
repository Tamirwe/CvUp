import { useContext } from "react";
import { RootStoreContext } from "../services/StoreProvider";

export const useStore = () => useContext(RootStoreContext);
