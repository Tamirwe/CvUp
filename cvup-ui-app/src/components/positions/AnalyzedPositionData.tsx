import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";

export const AnalyzedPositionData = observer(() => {
  const { positionsStore } = useStore();
  const positionId = positionsStore.editPosition?.id;
  const data = positionsStore.positionAnalyzedData;

  useEffect(() => {
    if (positionId) {
      positionsStore.getPositionAnalyzedData(positionId);
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  if (!data || data.positionId !== positionId) {
    return null;
  }

  return (
    <pre style={{ margin: 0, fontSize: 13, whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
      {JSON.stringify(data, null, 2)}
    </pre>
  );
});
