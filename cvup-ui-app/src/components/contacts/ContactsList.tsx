import {
  Fab,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Slide,
} from "@mui/material";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Hooks/useStore";
import { IContact } from "../../models/GeneralModels";
import { useRef, useState, useEffect, useCallback } from "react";
import { isMobile } from "react-device-detect";
import { MdKeyboardArrowUp } from "react-icons/md";

export const ContactsList = observer(() => {
  const { customersContactsStore, generalStore } = useStore();
  const listRef = useRef<any>(null);
  const [contactsList, setContactsList] = useState<IContact[]>([]);
  const [isScrolled, setIsScrolled] = useState(false);

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
    const sTop = instance.scrollTop;

    if (sTop > 50) {
      setIsScrolled(true);
    } else {
      setIsScrolled(false);
    }

    if (instance.scrollHeight - instance.clientHeight < sTop + 150) {
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
    <>
      <List
        ref={listRef}
        dense={true}
        sx={{
          height: isMobile ? "calc(100vh - 148px)" : "calc(100vh - 160px)",
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
                pr: "10px",
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
      <Slide direction="up" in={isScrolled} mountOnEnter unmountOnExit>
        <Fab
          size="medium"
          color="primary"
          sx={{
            zIndex: 999,
            position: "fixed",
            bottom: "7rem",
            left: "4rem",
            fontSize: "2rem",
          }}
          onClick={() => {
            listRef.current.scrollTop = 0;
          }}
        >
          <MdKeyboardArrowUp />
        </Fab>
      </Slide>
    </>
  );
});
