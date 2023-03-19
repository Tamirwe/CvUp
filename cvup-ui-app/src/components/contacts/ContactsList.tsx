import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Hooks/useStore";

export const ContactsList = observer(() => {
  const { customersContactsStore, generalStore } = useStore();

  return (
    <List
      dense={true}
      sx={{
        backgroundColor: "#fff",
        height: "calc(100vh - 81px)",
        overflowY: "hidden",
        "&:hover ": {
          overflow: "overlay",
        },
      }}
    >
      {customersContactsStore.contactsList &&
        customersContactsStore.contactsList.map((cont, i) => {
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
                  primary={`${cont.firstName} ${cont.lastName} - ${cont.customerName}`}
                  secondary={`${cont.email}, ${cont.phone}`}
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
