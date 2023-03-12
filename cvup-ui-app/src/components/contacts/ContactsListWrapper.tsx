import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { ContactsList } from "./ContactsList";

export const ContactsListWrapper = observer(() => {
  const { contactsStore } = useStore();

  useEffect(() => {
    if (!contactsStore.contactsList.length) {
      contactsStore.getContactsList();
    }
  }, []);

  return <ContactsList />;
});
