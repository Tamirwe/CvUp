import { List, ListItemButton, ListItemText } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { useLocation, useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { CvDisplayedListEnum } from "../../models/GeneralEnums";
import styles from "./CandsList.module.scss";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

interface IProps {
  candPosCvId?: number;
}

export const CandDupCvsList = observer(({ candPosCvId }: IProps) => {
  const { candsStore, generalStore } = useStore();
  let location = useLocation();
  const navigate = useNavigate();

  return (
    <List
      component="div"
      disablePadding
      dense={true}
      sx={{
        // backgroundColor: "#fffae2",
        border: "1px solid #ffdcdc",
        maxHeight: "208px",
        overflowY: "auto",
      }}
    >
      {/* <div style={{ fontWeight: 700, padding: "0.2rem" }}>Duplicates</div> */}
      {candsStore.candDupCvsList &&
        candsStore.candDupCvsList.map((dupCv, i) => {
          return (
            <ListItemButton
              title={candPosCvId === dupCv.cvId ? "Attached CV" : ""}
              key={`${dupCv.cvId}dup`}
              className={classNames({
                [styles.poslistItemCurrent]: candPosCvId === dupCv.cvId,
              })}
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

                candsStore.displayCvDuplicate(dupCv);
              }}
            >
              <div
                className={classNames({
                  [styles.listItem]: true,
                })}
                style={{ direction: "ltr", fontSize: "0.8rem" }}
              >
                <div
                  title="Cv sent date"
                  className={classNames({
                    [styles.listItemDate]: true,
                    [styles.isMobile]: isMobile,
                  })}
                >
                  {format(new Date(dupCv.cvSent), "MMM d, yyyy")}
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
                  {
                    dupCv.emailSubject.substring(
                      dupCv.emailSubject.indexOf(" למשרת ") + 6
                    )
                    // .replace("מועמדות חדשה מ", "")
                    // .replace(" למשרת ", " - ")
                  }
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
