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

export const UsersList = observer(() => {
  const { authStore, generalStore } = useStore();

  return (
    <List
      dense={true}
      sx={{
        backgroundColor: "#fff",
        // height: "calc(100vh - 81px)",
        overflowY: "hidden",
        "&:hover ": {
          overflow: "overlay",
        },
      }}
    >
      {authStore.usersList.map((usr, i) => {
        return (
          <ListItem
            key={usr.id}
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
              <ListItemText
                primary={usr.firstName}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
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
