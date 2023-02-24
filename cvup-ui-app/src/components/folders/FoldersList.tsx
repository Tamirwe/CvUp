import { List } from "@mui/material";
import { observer } from "mobx-react-lite";

export const FoldersList = observer(() => {
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
    ></List>
  );
});
