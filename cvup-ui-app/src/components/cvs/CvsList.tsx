import {
  Button,
  Hidden,
  Link,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";

export const CvsList = observer(() => {
  const { cvsStore } = useStore();

  useEffect(() => {
    cvsStore.getCvsList();
  }, []);

  return (
    <List dense={true} sx={{ maxWidth: "20rem" }}>
      {cvsStore.cvsList.map((cv, i) => {
        return (
          <ListItem key={cv.cvId}>
            <ListItemButton
              onClick={() => {
                cvsStore.setDoc(cv.encriptedId);
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
