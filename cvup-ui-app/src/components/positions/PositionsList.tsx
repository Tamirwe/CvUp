import {
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { MdBookmarkBorder, MdBookmark } from "react-icons/md";

export const PositionsList = observer(() => {
  const { positionsStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      await positionsStore.getPositionsList(false);
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
          <ListItem
            key={pos.id}
            dense
            disablePadding
            secondaryAction={
              <Checkbox
                icon={<MdBookmarkBorder />}
                checkedIcon={<MdBookmark />}
              />
            }
          >
            <ListItemButton
              selected={pos.id === positionsStore.currentPosition.id}
              onClick={() => {
                navigate(`/position/${pos.id}`);

                // positionsStore.GetPosition(pos.id);
              }}
            >
              <ListItemText primary={pos.name} sx={{ pr: 5 }} />
              <ListItemText
                primary={pos.updated.toString()}
                sx={{
                  textAlign: "right",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                }}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
