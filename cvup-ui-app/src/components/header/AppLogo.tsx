interface IProps {
  height?: number;
  title?: string;
  onClick?: () => void;
}

// The logo as markup instead of CvUpLogo2.png. The png carries a baked-in white
// rectangle, which shows as a bright patch on any surface that is not white --
// the admin ground in particular. Drawn here it is transparent, stays sharp at
// any size and on any screen, and costs no request.
//
// The mark keeps the brand's two ridges; the wordmark pairs the app's slate ink
// with its green.
const RIDGE_BACK = "#9aa8bd";
const RIDGE_FRONT = "#5b6b84";
const SNOW = "#e9eef4";
const WORD_DARK = "#2f3b45";
const WORD_GREEN = "#087d56";

// The name, split between the two inks. The viewBox is sized to the rendered
// wordmark -- widen VIEW_W if the name gets longer, or the right edge clips.
const NAME_DARK = "HaMa";
const NAME_GREEN = " A reHet";
const VIEW_W = 280;
const VIEW_H = 40;

export const AppLogo = ({ height = 32, title, onClick }: IProps) => {
  return (
    <svg
      viewBox={`0 0 ${VIEW_W} ${VIEW_H}`}
      height={height}
      width={(height * VIEW_W) / VIEW_H}
      role="img"
      aria-label={`${NAME_DARK}${NAME_GREEN}`}
      onClick={onClick}
      style={{
        display: "block",
        cursor: onClick ? "pointer" : "default",
        overflow: "visible",
      }}
    >
      {title && <title>{title}</title>}
      {/* Two ridges, the back one lighter so the peaks read at small sizes. */}
      <path d="M2 32 L16 11 L30 32 Z" fill={RIDGE_BACK} />
      <path d="M13 32 L28 5 L43 32 Z" fill={RIDGE_FRONT} />
      {/* Snow cap. A light tint rather than white: white read as a crack in the
          peak when the mark is blown up. */}
      <path
        d="M28 5 L33 14 L30.5 12.2 L28 14.4 L25.5 12.2 L23 14 Z"
        fill={SNOW}
      />
      <text
        x="52"
        y="31"
        // Without this the space between the two tspans is collapsed away and
        // the name runs together.
        xmlSpace="preserve"
        fontFamily="inherit"
        fontSize="26"
        fontWeight="700"
        letterSpacing="-0.5"
      >
        <tspan fill={WORD_DARK}>{NAME_DARK}</tspan>
        <tspan fill={WORD_GREEN}>{NAME_GREEN}</tspan>
      </text>
    </svg>
  );
};
