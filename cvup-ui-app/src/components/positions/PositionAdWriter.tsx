import { observer } from "mobx-react";
import { useStore } from "../../Hooks/useStore";

export const PositionAdWriter = observer(() => {
  const { positionsStore } = useStore();

  if (!positionsStore.positionAiRewriteData) {
    return null;
  }

  return (
    <pre style={{ margin: 0, fontSize: 13, whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
      {positionsStore.positionAiRewriteData}
    </pre>
  );
});
