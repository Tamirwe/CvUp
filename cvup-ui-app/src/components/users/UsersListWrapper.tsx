import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { UsersList } from "./UsersList";

export const UsersListWrapper = observer(() => {
  const { authStore } = useStore();

  useEffect(() => {
    if (!authStore.usersList.length) {
      authStore.getUsersList();
    }
  }, []);

  return <UsersList />;
});
