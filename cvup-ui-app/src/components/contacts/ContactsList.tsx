import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Hooks/useStore";
import { IContact } from "../../models/GeneralModels";
import { useRef, useState, useEffect, useCallback } from "react";

export const ContactsList = observer(() => {
  const { customersContactsStore, generalStore } = useStore();
  const listRef = useRef<any>(null);
  const [contactsList, setContactsList] = useState<IContact[]>([]);

  useEffect(() => {
    if (!customersContactsStore.contactsListSorted.length) {
      customersContactsStore.getContactsList();
    }
  }, []);

  useEffect(() => {
    // if (customersContactsStore.contactsListSorted.length > 0) {
    setContactsList(customersContactsStore.contactsListSorted?.slice(0, 50));
    // }
  }, [customersContactsStore.contactsListSorted]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    if (
      instance.scrollHeight - instance.clientHeight <
      instance.scrollTop + 150
    ) {
      if (contactsList) {
        const numRecords = contactsList.length;
        const newPosList = contactsList.concat(
          customersContactsStore.contactsListSorted?.slice(
            numRecords,
            numRecords + 50
          )
        );
        setContactsList(newPosList);
      }

      console.log(instance.scrollTop);
    }
  }, [contactsList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  return (
    <List
      ref={listRef}
      dense={true}
      sx={{
        backgroundColor: "#fff",
        height: "calc(100vh - 81px)",
        overflowY: "scroll",
        // "&:hover ": {
        //   overflow: "overlay",
        // },
      }}
    >
      {contactsList.map((cont, i) => {
        return (
          <ListItem
            key={cont.id}
            dense
            disablePadding
            component="nav"
            sx={{
              flexDirection: "column",
              alignItems: "normal",
              pl: "10px",
            }}
          >
            <ListItemButton
              sx={{ pr: "4px", pl: "4px" }}
              // selected={
              //   cand.candidateId === candsStore.candDisplay?.candidateId
              // }
              onClick={() => {
                customersContactsStore.selectedContact = cont;
                generalStore.showContactFormDialog = true;
                // if (location.pathname !== "/cv") {
                //   navigate(`/cv`);
                // }
                // candsStore.displayCvMain(cand);
              }}
            >
              <ListItemText
                primary={`${cont.firstName} ${
                  cont.lastName ? cont.lastName : ""
                } - ${cont.customerName}`}
                secondary={`${cont.email}, ${cont.phone ? cont.phone : ""}`}
                sx={{
                  textAlign: "right",
                  fontSize: "0.775rem",
                  alignSelf: "start",
                  whiteSpace: "nowrap",
                }}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
