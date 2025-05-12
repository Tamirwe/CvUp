import {
  Checkbox,
  Fab,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Slide,
} from "@mui/material";
import { observer } from "mobx-react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import {
  MdKeyboardArrowUp,
  MdOutlineEdit,
  MdPersonAddAlt1,
  MdPersonRemove,
} from "react-icons/md";
import { format } from "date-fns";
import styles from "./PositionsList.module.scss";
import {
  AlertConfirmDialogEnum,
  TabsCandsEnum,
} from "../../models/GeneralEnums";
import { IPosition } from "../../models/GeneralModels";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

export const PositionsList = observer(() => {
  const { positionsStore, candsStore, generalStore } = useStore();

  const listRef = useRef<any>(null);
  const [posList, setPosList] = useState<IPosition[]>([]);
  const [isScrolled, setIsScrolled] = useState(false);

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
    const sTop = instance.scrollTop;

    if (sTop > 50) {
      setIsScrolled(true);
    } else {
      setIsScrolled(false);
    }

    if (instance.scrollHeight - instance.clientHeight < sTop + 150) {
      if (posList) {
        const numRecords = posList.length;
        const newPosList = posList.concat(
          positionsStore.sortedPosList?.slice(numRecords, numRecords + 50)
        );
        setPosList(newPosList);
      }
    }
  }, [posList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  const handleEditPositionClick = useCallback(
    async (event: React.MouseEvent<HTMLDivElement | HTMLAnchorElement>) => {
      event.stopPropagation();
      event.preventDefault();

      await positionsStore.getPosition(
        positionsStore.selectedPosition?.id || 0
      );
      generalStore.showPositionFormDialog = true;

      if (isMobile) {
        generalStore.leftDrawerOpen = false;
      }
    },
    []
  );

  const handleAttachDeattach = useCallback(
    async (event: React.ChangeEvent<HTMLInputElement>, posId: number) => {
      if (event.target.checked) {
        await candsStore.attachPosCandCv(posId);
        if (candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands) {
          await positionsStore.positionClick(posId, true);
          positionsStore.setRelatedPositionToCandDisplay();
          candsStore.setDisplayCandOntopPCList();
          candsStore.currentTabCandsLists = TabsCandsEnum.PositionCands;
        }
        generalStore.alertSnackbar("success", "Candidate attached to position");
      } else {
        const isDelete = await generalStore.alertConfirmDialog(
          AlertConfirmDialogEnum.Confirm,
          "Detach Candidate from position",
          "Are you sure you want to detach the candidate from  this position?"
        );

        if (isDelete) {
          await candsStore.detachPosCand(candsStore.candDisplay, posId);
          generalStore.alertSnackbar(
            "success",
            "Candidate detached from position."
          );
        }
      }
    },
    []
  );

  return (
    <>
      <List
        ref={listRef}
        className={classNames({
          [styles.posList]: true,
          [styles.isMobile]: isMobile,
        })}
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
                  <span title="updated">
                    {pos.updated &&
                      format(new Date(pos.updated), "MMM d, yyyy")}
                  </span>
                  <br></br>
                  <span title="created">
                    {pos.created &&
                      format(new Date(pos.created), "MMM d, yyyy")}
                  </span>
                </div>
                <ListItemText
                  primary={pos.name}
                  secondary={
                    <span
                      style={{ display: "flex", flexDirection: "row-reverse" }}
                    >
                      <span>{pos.customerName}</span>
                      {pos.candsCount && (
                        <span
                          style={{ fontSize: "0.7rem", fontWeight: "bold" }}
                        >
                          {`(${pos.candsCount})  `}&nbsp;
                        </span>
                      )}
                    </span>
                  }
                  sx={{
                    textAlign: "right",
                    paddingRight: 2,
                  }}
                />
                {positionsStore.selectedPosition?.id === pos.id && (
                  <ListItemIcon
                    onClick={(event) => handleEditPositionClick(event)}
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
                    >
                      <MdOutlineEdit />
                    </IconButton>
                  </ListItemIcon>
                )}
                <Checkbox
                  checked={
                    candsStore.candDisplay &&
                    candsStore.candDisplay.candPosIds &&
                    candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                      ? true
                      : false
                  }
                  // disabled={
                  //   candsStore.candDisplay &&
                  //   candsStore.candDisplay.candPosIds &&
                  //   candsStore.candDisplay.candPosIds.indexOf(pos.id) > -1
                  // }
                  onChange={(event) => {
                    handleAttachDeattach(event, pos.id);
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
                  checkedIcon={<MdPersonRemove />}
                />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>
      <Slide direction="up" in={isScrolled} mountOnEnter unmountOnExit>
        <Fab
          size="medium"
          color="primary"
          sx={{
            zIndex: 999,
            position: "fixed",
            bottom: "7rem",
            left: "4rem",
            fontSize: "2rem",
          }}
          onClick={() => {
            listRef.current.scrollTop = 0;
          }}
        >
          <MdKeyboardArrowUp />
        </Fab>
      </Slide>
    </>
  );
});
