import SideBar from "./sidebar/session-sidebar";
import './session.css';
import Main from './main/main';
import { useAtom, useAtomValue } from "jotai";
import { exercisesAtomAtoms, selectedExerciseIndexAtom, sessionAtom } from "./shared/session.atoms";
import { useMemo } from "react";

const Session = () => {
    const session = useAtomValue(sessionAtom);
    const selectedIndex = useAtomValue(selectedExerciseIndexAtom);
    const [exercisesAtoms] = useAtom(exercisesAtomAtoms);
    const selectedExerciseAtom = useMemo(() => exercisesAtoms[selectedIndex], [selectedIndex]);
    
    //console.dir(session)

    return (
        <>
            <div className="container-fluid d-flex">
                <SideBar></SideBar>
                <Main selectedExerciseAtom={selectedExerciseAtom}></Main>
            </div>
        </>

    )
}

export default Session;