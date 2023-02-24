import { observer } from "mobx-react";
import { ContactsList } from "./ContactsList";

export const ContactsListWrapper = observer(() => {
  return <ContactsList />;
});
