import exp from "constants"
import { atom, useAtom } from "jotai";
import { useUpdateAtom } from "jotai/utils";
import { focusAtom } from 'jotai/optics'
import { sessionAtom } from "../../shared/session.atoms";
import { ExerciseVM, SessionVM, SetMetrics } from "../../shared/session.model";
import { useCallback, useRef } from "react";

const showAddAtom = atom(false);
const exercisesAtom = focusAtom(sessionAtom, (optic) => optic.prop('exercises'))
const selectedExerciseAtom = atom("");

const AddExercise = () => {
    const [showAdd, setShowAdd] = useAtom(showAddAtom);
    const [selectedExercise, setSelectedExercise] = useAtom(selectedExerciseAtom);
    const setExercise = useUpdateAtom (exercisesAtom);

    const handleAddExercise = (x: string) => {
        const y = "Test" + x;
        const newEx = {completed: false, sets: [], exercise: {name: y, metricType: "weight", targetSets: []}} as ExerciseVM<SetMetrics>;
        setExercise(exercises => [...exercises, newEx]);
        setShowAdd(false);
    }

    const handleShowAddExercise = () => {
        setShowAdd(true);
    }

    const addButton = (
        <li className="list-group-item">
            <i onClick={() => setShowAdd(true)} className="bi bi-plus-circle-fill fs-3"></i>
        </li>
    )

    const showListRef = useCallback((node: any) => {
        if (node !== null) {
            node.focus();
        }
      }, []);

    const addList = (
        <div className="input-group">
            <select ref={showListRef} className="form-select" id="inputGroupSelect01" value={selectedExercise} onChange={(e) => handleAddExercise(e.target.value)} onBlur={() => setShowAdd(false)} >
                <option value="">Choose...</option>
                <option value="1">Something Heavy</option>
                <option value="2" >Incline Barbell Bench Press</option>
                <option value="3">Incline Dumbbell Bench Press</option>
            </select>
        </div>
    );

    return showAdd ? addList : addButton;
}

export default AddExercise;