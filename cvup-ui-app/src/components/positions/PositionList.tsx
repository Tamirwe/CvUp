import { List } from "@mui/material";

export const PositionList = () => {
  return (
    <List dense={true} sx={{ maxWidth: "20rem" }}>
      {/* {cvsStore.cvsList.map((cv, i) => {
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
    })} */}
    </List>
  );
};
