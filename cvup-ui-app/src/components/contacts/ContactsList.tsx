import {
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Hooks/useStore";

export const ContactsList = observer(() => {
  const { contactsStore, generalStore } = useStore();

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
      {contactsStore.contactsList.map((cont, i) => {
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
                generalStore.showContactFormDialog = true;
                // if (location.pathname !== "/cv") {
                //   navigate(`/cv`);
                // }
                // candsStore.displayCvMain(cand);
              }}
            >
              {/* <ListItemText
                primary={format(new Date(cand.cvSent), "MMM d, yyyy")}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                  alignSelf: "start",
                  whiteSpace: "nowrap",
                }}
              /> */}
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
