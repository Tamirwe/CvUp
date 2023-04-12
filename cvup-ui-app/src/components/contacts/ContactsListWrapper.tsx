import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { ContactsList } from "./ContactsList";
import { Box } from "@mui/material";
import { SearchControl } from "../header/SearchControl";

export const ContactsListWrapper = () => {
  const handleContactsSearch = (val: string) => {};

  return (
    <Box>
      <Box mt={1} ml={1}>
        <SearchControl onSearch={handleContactsSearch} />
      </Box>
      <ContactsList />;
    </Box>
  );
};
