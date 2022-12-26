import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";

export const CvsList = observer(() => {
  const { cvsStore } = useStore();

  useEffect(() => {
    cvsStore.getCvsList();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      dense={true}
      sx={{
        backgroundColor: "#fff",
      }}
    >
      {cvsStore.cvsList.map((cv, i) => {
        return (
          <ListItem key={cv.cvId}>
            <ListItemButton
              onClick={() => {
                cvsStore.setDoc(cv.cvId);
              }}
            >
              <ListItemText primary={cv.emailSubject} />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
