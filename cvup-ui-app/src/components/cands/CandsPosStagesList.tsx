import { IconButton } from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import { MdRemove } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { CandsSourceEnum } from "../../models/GeneralEnums";
import { ICand } from "../../models/GeneralModels";
import styles from "./CandsList.module.scss";
import classNames from "classnames";
import { usePositionClick } from "../../Hooks/usePositionClick";

interface IProps {
  candsSource: CandsSourceEnum;
  cand: ICand;
}

export const CandsPosStagesList = observer(({ candsSource, cand }: IProps) => {
  const { candsStore, positionsStore } = useStore();
  const handlePositionClick = usePositionClick();

  return (
    <div style={{ fontSize: "0.775rem", paddingRight: "1rem" }}>
      {cand.posStages &&
        candsStore.sortPosStage(cand.posStages).map((stage, i) => {
          return (
            <div
              key={i}
              style={{
                display: "flex",
                flexDirection: "row-reverse",
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
                    handlePositionClick(stage.id, cand.candidateId);
                  },
                })}
              >
                <div>{format(new Date(stage.d), "dd/MM/yyyy")}</div>
                <div>-</div>
                <div
                  style={{
                    direction: "rtl",
                    overflow: "hidden",
                    whiteSpace: "nowrap",
                    textOverflow: "ellipsis",
                    paddingLeft: "1rem",
                    maxWidth: "21rem",
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
