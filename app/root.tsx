import RemixRoot, {
  getRootLinks,
  getRootMeta,
  RootCatchBoundary,
} from "@dvargas92495/app/components/RemixRoot";
import remixRootLoader from "@dvargas92495/app/utils/remixRootLoader.server";
import styles from "./tailwind.css";

export const loader = remixRootLoader;
export const meta = getRootMeta({ title: "App" });
export const links = getRootLinks([{ rel: "stylesheet", href: styles }]);
export const CatchBoundary = RootCatchBoundary;
export default RemixRoot;