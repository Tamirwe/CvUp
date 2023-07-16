import { List, ListItemButton, ListItemText } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { CvDisplayedListEnum } from "../../models/GeneralEnums";
import styles from "./CandsList.module.scss";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

export const CandDupCvsList = observer(() => {
  const { candsStore, generalStore } = useStore();
  let location = useLocation();
  const navigate = useNavigate();

  return (
    <List
      component="div"
      disablePadding
      dense={true}
      sx={{
        backgroundColor: "#fbfbfb",
        border: "1px solid #ffdcdc",
        maxHeight: "300px",
        overflowY: "scroll",

        // "&:hover ": {
        //   overflow: "overlay",
        // },
      }}
    >
      {candsStore.candDupCvsList &&
        candsStore.candDupCvsList.map((dupCv, i) => {
          return (
            <ListItemButton
              key={`${dupCv.cvId}dup`}
              sx={{
                "&.Mui-selected": {
                  backgroundColor: "#edf1ff",
                },
              }}
              selected={dupCv.cvId === candsStore.candDupSelected?.cvId}
              onClick={() => {
                if (location.pathname !== "/cv") {
                  navigate(`/cv`);
                }

                if (isMobile) {
                  generalStore.rightDrawerOpen = false;
                }

                candsStore.displayCvDuplicate(
                  dupCv,
                  CvDisplayedListEnum.CandsList
                );
              }}
            >
              <div
                className={classNames({
                  [styles.listItem]: true,
                })}
                style={{ direction: "ltr" }}
              >
                <div
                  className={classNames({
                    [styles.listItemDate]: true,
                    [styles.isMobile]: isMobile,
                  })}
                >
                  {format(new Date(dupCv.cvSent), "dd/MM/yyyy")}
                </div>
                <div>&nbsp;-&nbsp;</div>
                <div
                  style={{
                    direction: "rtl",
                    overflow: "hidden",
                    whiteSpace: "nowrap",
                    textOverflow: "ellipsis",
                    paddingLeft: "1rem",
                  }}
                >
                  {dupCv.emailSubject}
                </div>
              </div>

              {/* <ListItemText
                primary={format(new Date(dupCv.cvSent), "MMM d, yyyy")}
                sx={{
                  whiteSpace: "nowrap",
                  "& span": { fontSize: "0.75rem" },
                }}
              />
              <ListItemText
                sx={{
                  "& span, p": { fontSize: "0.75rem", textAlign: "right" },
                }}
                primary={dupCv.emailSubject}
                // secondary={dupCv.emailSubject}
              /> */}
            </ListItemButton>
          );
        })}
    </List>
  );
});
