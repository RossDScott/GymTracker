import { atom, useAtom } from "jotai";
import { DateTime } from "luxon";
import { useEffect } from "react";

const clockAtom = atom(DateTime.now());

const CurrentTime = () => {
    const [clock, setClock] = useAtom(clockAtom);

    useEffect(() => {
        const timer = setInterval(() => setClock(DateTime.now()), 1000);
        return () => clearInterval(timer);
    }, [setClock]);

    return (
        <>
            <span className="fw-semibold">Current Time</span>
            <h1>{clock.toLocaleString(DateTime.TIME_24_SIMPLE)}</h1>
        </>
    );
}

export default CurrentTime;