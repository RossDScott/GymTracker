import exp from "constants"
import { atom, useAtom } from "jotai";
import { useUpdateAtom } from "jotai/utils";
import { exercisesAtom } from "../../shared/session.atoms";
import { ExerciseVM, SetMetrics } from "../../shared/session.model";
import { useCallback, useRef } from "react";

const showAddAtom = atom(false);

const AddExercise = () => {
    const [showAdd, setShowAdd] = useAtom(showAddAtom);
    const setExercise = useUpdateAtom(exercisesAtom);

    const handleAddExercise = (name: string) => {
        const newEx = {completed: false, sets: [], exercise: {name: name, metricType: "weight", targetSets: []}} as ExerciseVM<SetMetrics>;
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
            <select ref={showListRef} className="form-select me-2" onChange={(e) => handleAddExercise(e.target.value)} onBlur={() => setShowAdd(false)} >
                <option value="">Choose...</option>
                <option value="Something Heavy">Something Heavy</option>
                <option value="Incline Barbell Bench Press" >Incline Barbell Bench Press</option>
                <option value="Incline Dumbbell Bench Press">Incline Dumbbell Bench Press</option>
            </select>
        </div>
    );

    return showAdd ? addList : addButton;
}

export default AddExercise;