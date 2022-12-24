import { List, ListItem, ListItemButton, ListItemText } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";

export const PositionsList = observer(() => {
  const { positionsStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      positionsStore.getPositionsList(false);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      sx={{
        width: "98%",
      }}
    >
      {positionsStore.positionsList?.map((pos, i) => {
        return (
          <ListItem key={pos.id} disablePadding>
            <ListItemButton
              selected={pos.id === positionsStore.currentPosition.id}
              onClick={() => {
                navigate(`/position/${pos.id}`);

                // positionsStore.GetPosition(pos.id);
              }}
            >
              <ListItemText primary={pos.name} />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
