import {
  FormControlLabel,
  List,
  ListItemButton,
  ListItemText,
  Switch,
  Typography,
} from "@mui/material";
import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IUser } from "../../models/AuthModels";
import { PermissionTypeEnum, UserActiveEnum } from "../../models/GeneralEnums";

export const UsersList = observer(() => {
  const { authStore, generalStore } = useStore();
  const [userActive, setUserActive] = useState(false);

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
            <ListItemText
              primary={`${item.firstName} ${item.lastName}`}
              secondary={
                <Typography variant="caption" display="block" gutterBottom>
                  <b>Role:</b> {PermissionTypeEnum[item.permissionType]}
                  <b>, Status: </b>
                  {UserActiveEnum[item.activeStatus].replace("_", " ")}
                </Typography>
              }
            />
          </ListItemButton>
        );
      })}
    </List>
  );
});

// {`Role: ${
//   PermissionTypeEnum[item.permissionType]
// }, Status: ${UserActiveEnum[item.activeStatus].replace(
//   "_",
//   " "
// )}`}
