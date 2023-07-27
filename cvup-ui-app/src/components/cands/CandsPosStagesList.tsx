import { IconButton } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { MdRemove } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { CandsSourceEnum } from "../../models/GeneralEnums";
import { ICand } from "../../models/GeneralModels";
import styles from "./CandsList.module.scss";
import classNames from "classnames";
import { isMobile } from "react-device-detect";

interface IProps {
  candsSource: CandsSourceEnum;
  cand: ICand;
}

export const CandsPosStagesList = observer(({ candsSource, cand }: IProps) => {
  const { candsStore, positionsStore } = useStore();

  return (
    <div
      style={{ direction: "ltr", fontSize: "0.775rem", paddingRight: "0.2rem" }}
    >
      {cand.posStages &&
        candsStore.sortPosStage(cand.posStages).map((stage, i) => {
          return (
            <div
              key={i}
              style={{
                display: "flex",
                flexDirection: "row-reverse",
                // paddingRight: "2.5rem",
                alignItems: "center",
              }}
            >
              {cand.candidateId === candsStore.candDisplay?.candidateId && (
                <IconButton
                  size="small"
                  sx={{ ml: 1 }}
                  onClick={(event) => {
                    event.stopPropagation();
                    event.preventDefault();
                    candsStore.detachPosCand(cand, stage.id, i);
                  }}
                >
                  <MdRemove />
                </IconButton>
              )}{" "}
              <div
                className={classNames({
                  [styles.listItem]: true,
                  [styles.listItemSelected]:
                    cand.candidateId === candsStore.candDisplay?.candidateId,
                })}
                style={{
                  color: candsStore.findStageColor(stage.t),
                }}
                title={candsStore.findStageName(stage.t)}
                {...(cand.candidateId ===
                  candsStore.candDisplay?.candidateId && {
                  onClick: (event) => {
                    event.stopPropagation();
                    event.preventDefault();
                    positionsStore.positionClick(stage.id);
                  },
                })}
              >
                <div
                  className={classNames({
                    [styles.listItemDate]: true,
                    [styles.isMobile]: isMobile,
                  })}
                >
                  {format(new Date(stage.d), "dd/MM/yyyy")}
                </div>
                <div>&nbsp;-&nbsp;</div>
                <div
                  style={{
                    direction: "rtl",
                    overflow: "hidden",
                    whiteSpace: "nowrap",
                    textOverflow: "ellipsis",
                    paddingLeft: "1rem",

                    textDecoration:
                      candsSource === CandsSourceEnum.Position &&
                      stage.id === positionsStore.selectedPosition?.id
                        ? "underline"
                        : "none",
                  }}
                >
                  {positionsStore.findPosName(stage.id)}
                </div>
              </div>
            </div>
          );
        })}
    </div>
  );
});
