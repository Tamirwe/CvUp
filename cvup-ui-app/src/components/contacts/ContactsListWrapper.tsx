import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { ContactsList } from "./ContactsList";

export const ContactsListWrapper = observer(() => {
  const { customersContactsStore } = useStore();

  useEffect(() => {
    if (!customersContactsStore.contactsList.length) {
      customersContactsStore.getContactsList();
    }
  }, []);

  return <ContactsList />;
});
