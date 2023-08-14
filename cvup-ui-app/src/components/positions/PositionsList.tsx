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
import { CandsSourceEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { BsFillPersonFill } from "react-icons/bs";
import { IPosition } from "../../models/GeneralModels";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

export const PositionsList = observer(() => {
  const { positionsStore, candsStore, generalStore } = useStore();

  const listRef = useRef<any>(null);
  const [posList, setPosList] = useState<IPosition[]>([]);

  const handleAttachPosCandCv = async (posId: number) => {
    await candsStore.attachPosCandCv(posId);
    await positionsStore.positionClick(posId, true);
    positionsStore.setRelatedPositionToCandDisplay();
    candsStore.setDisplayCandOntopPCList();
    candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
  };

  useEffect(() => {
    if (positionsStore.sortedPosList) {
      setPosList(positionsStore.sortedPosList?.slice(0, 50));

      if (positionsStore.isSelectedPositionOnTop) {
        listRef.current.scrollTop = 0;
      }
    }
  }, [positionsStore.sortedPosList]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    if (
      instance.scrollHeight - instance.clientHeight <
      instance.scrollTop + 150
    ) {
      if (posList) {
        const numRecords = posList.length;
        const newPosList = posList.concat(
          positionsStore.sortedPosList?.slice(numRecords, numRecords + 50)
        );
        setPosList(newPosList);
      }

      // console.log(instance.scrollTop);
    }
  }, [posList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  const handleEditPositionClick = () => {};

  return (
    <List
      ref={listRef}
      className={classNames({
        [styles.posList]: true,
        [styles.isMobile]: isMobile,
      })}

      // sx={{
      //   backgroundColor: "#fff",
      //   height: "calc(100vh - 96px)",
      //   overflowY: "scroll",
      //   // "&:hover ": {
      //   //   overflow: "overlay",
      //   // },
      // }}
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
              onClick={() => positionsStore.positionClick(pos.id)}
            >
              <div
                className={classNames({
                  [styles.listItemDate]: true,
                  [styles.isMobile]: isMobile,
                })}
              >
                {pos.updated && format(new Date(pos.updated), "MMM d, yyyy")}
              </div>
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

                      if (isMobile) {
                        generalStore.leftDrawerOpen = false;
                      }
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
