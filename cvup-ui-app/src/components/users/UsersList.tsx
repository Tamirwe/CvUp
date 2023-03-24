import { List, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { IUser } from "../../models/AuthModels";

export const UsersList = observer(() => {
  const { authStore, generalStore } = useStore();

  useEffect(() => {
    (async () => {
      await authStore.getUsersList();
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleEditClick = (user: IUser) => {
    authStore.selectedUser = user;
    generalStore.showUserFormDialog = true;
  };
  return (
    <List
      sx={{
        width: "100%",
        bgcolor: "background.paper",
        position: "relative",
        overflow: "auto",
        maxHeight: 300,
      }}
    >
      {authStore.usersList.map((item, i) => {
        return (
          <ListItemButton
            onClick={() => handleEditClick(item)}
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{`${item.firstName} ${item.lastName}`}</ListItemText>
          </ListItemButton>
        );
      })}
    </List>
  );
});
