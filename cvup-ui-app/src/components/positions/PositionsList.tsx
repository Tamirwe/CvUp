import {
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdPersonAddAlt1 } from "react-icons/md";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { TabsCandsEnum } from "../../models/GeneralEnums";
import { BsFillPersonFill } from "react-icons/bs";

export const PositionsList = observer(() => {
  const { positionsStore, candsStore } = useStore();

  const handleAttachPosCandCv = (posId: number) => {
    candsStore.attachPosCandCv(posId);
    positionsStore.setPosSelected(posId);
    candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
  };

  const handlePositionClick = (posId: number) => {
    if (positionsStore.selectedPosition?.id !== posId) {
      positionsStore.setPosSelected(posId);
      candsStore.getPositionCands(posId);
      candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
    }
  };

  useEffect(() => {
    (async () => {
      if (positionsStore.positionsList.length === 0) {
        await positionsStore.getPositionsList();
      }
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      sx={{
        overflowY: "auto",
        width: "98%",
        "&:hover": {},
      }}
    >
      {positionsStore.positionsList?.map((pos, i) => {
        return (
          <ListItem
            key={pos.id}
            dense
            disablePadding
            className={styles.showAddCv}
            sx={{
              "& .MuiListItemSecondaryAction-root": { right: 0 },
              flexDirection: "column",
              alignItems: "normal",
              pl: "18px",
            }}
          >
            <ListItemButton
              sx={{ pl: "4px" }}
              selected={pos.id === positionsStore.selectedPosition?.id}
              onClick={() => handlePositionClick(pos.id)}
            >
              <ListItemText
                primary={
                  pos.updated && format(new Date(pos.updated), "MMM d, yyyy")
                }
                sx={{
                  textAlign: "right",
                  alignSelf: "start",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                }}
              />
              <ListItemText primary={pos.name} />
              <Checkbox
                checked={
                  candsStore.candDisplay && candsStore.candDisplay.candPosIds
                    ? candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                    : false
                }
                disabled={
                  candsStore.candDisplay &&
                  candsStore.candDisplay.candPosIds &&
                  candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                }
                onChange={(event) => {
                  handleAttachPosCandCv(pos.id);
                }}
                onClick={(event) => {
                  event.stopPropagation();
                }}
                sx={{
                  right: 0,
                  marginRight: "2px",
                  color: "#d7d2d2",
                  "& svg": { fontSize: 22 },
                  "&.Mui-checked": {
                    color: "#ff8d00",
                  },
                }}
                icon={<MdPersonAddAlt1 />}
                checkedIcon={<BsFillPersonFill />}
              />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
});
