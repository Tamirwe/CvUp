import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { UsersList } from "./UsersList";

export const UsersListWrapper = observer(() => {
  const { contactsStore } = useStore();

  useEffect(() => {
    if (!contactsStore.contactsList.length) {
      contactsStore.getContactsList();
    }
  }, []);

  return <UsersList />;
});
