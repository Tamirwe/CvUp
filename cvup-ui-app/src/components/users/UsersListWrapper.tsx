import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { UsersList } from "./UsersList";

export const UsersListWrapper = observer(() => {
  const { customersContactsStore } = useStore();

  useEffect(() => {
    if (!customersContactsStore.contactsList.length) {
      customersContactsStore.getContactsList();
    }
  }, []);

  return <UsersList />;
});
