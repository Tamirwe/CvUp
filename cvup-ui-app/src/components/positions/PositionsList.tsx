import {
  Checkbox,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { observer } from "mobx-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineEdit, MdPersonAddAlt1, MdRemove } from "react-icons/md";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import { TabsCandsEnum } from "../../models/GeneralEnums";
import { BsFillPersonFill } from "react-icons/bs";
import { IPosition } from "../../models/GeneralModels";
import { usePositionClick } from "../../Hooks/usePositionClick";

export const PositionsList = observer(() => {
  const { positionsStore, candsStore, generalStore } = useStore();
  const handlePositionClick = usePositionClick();

  const listRef = useRef<any>(null);
  const [posList, setPosList] = useState<IPosition[]>([]);

  const handleAttachPosCandCv = (posId: number) => {
    candsStore.attachPosCandCv(posId);
    positionsStore.setPosSelected(posId);
    candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
  };

  useEffect(() => {
    if (positionsStore.positionsSorted) {
      setPosList(positionsStore.positionsSorted?.slice(0, 50));
    }
  }, [positionsStore.positionsSorted]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    // if (
    //   instance.scrollHeight - instance.clientHeight <
    //   instance.scrollTop + 150
    // ) {
    //   if (posList) {
    //     const numRecords = posList.length;
    //     const newPosList = posList.concat(
    //       positionsStore.positionsSorted?.slice(numRecords, numRecords + 50)
    //     );
    //     setPosList(newPosList);
    //   }

    //   console.log(instance.scrollTop);
    // }
  }, [posList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  // const handlePositionClick = (posId: number) => {
  //   if (
  //     candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands ||
  //     positionsStore.selectedPosition?.id !== posId
  //   ) {
  //     positionsStore.setPosSelected(posId);
  //     candsStore.getPositionCands();
  //     candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
  //   }
  // };

  const handleEditPositionClick = () => {};

  return (
    <List
      ref={listRef}
      sx={{
        backgroundColor: "#fff",
        height: "calc(100vh - 96px)",
        overflowY: "scroll",
        // "&:hover ": {
        //   overflow: "overlay",
        // },
      }}
    >
      {posList.map((pos, i) => {
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
                  textAlign: "left",
                  alignSelf: "start",
                  color: "#bcc9d5",
                  fontSize: "0.775rem",
                }}
              />
              <ListItemText
                primary={pos.name}
                secondary={pos.customerName}
                sx={{
                  textAlign: "right",
                  paddingRight: 2,
                }}
              />
              {positionsStore.selectedPosition?.id === pos.id && (
                <ListItemIcon
                  onClick={(event) => {
                    event.stopPropagation();
                    handleEditPositionClick();
                  }}
                >
                  <IconButton
                    sx={{
                      right: 0,
                      marginRight: 1,
                      color: "#d7d2d2",
                      "&:hover ": {
                        color: "#ffab55",
                      },
                    }}
                    color="primary"
                    aria-label="upload picture"
                    component="label"
                    onClick={async () => {
                      await positionsStore.getPosition(
                        positionsStore.selectedPosition?.id || 0
                      );
                      generalStore.showPositionFormDialog = true;
                    }}
                  >
                    <MdOutlineEdit />
                  </IconButton>
                </ListItemIcon>
              )}
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
