import { Component, OnInit } from '@angular/core';
import { ExerciseVM, SessionVM, ExerciseSetVM, SetMetrics, SetWeightMetrics, SetTimeMetrics } from './session.model';
import { SessionService } from './session.service';
import { DateTime, Duration } from "luxon";

@Component({
    selector: 'app-session',
    templateUrl: './session.component.html',
    styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit {
    currentTime = DateTime.now();
    workoutTime: Duration = Duration.fromMillis(0);
    stopWatch: Duration | null = null;

    session: SessionVM | null = null;

    editRow = false;
    rowConfirmed = false;
    
    constructor(private sessionService: SessionService) { 
        
        setInterval(()=>{
            this.currentTime = DateTime.now();
            if(!this.session)
                return;

            this.workoutTime = DateTime.now().diff(this.session!.workoutStart, ['hours', 'minutes']);
        }, 1000);
    }

    ngOnInit(): void {
        this.session = this.sessionService.setupFakeSession();
        //this.selectedExercise = this.session.exercises[0];
    }

    handleConfirm(event: Event){
        event.cancelBubble = true;
        this.rowConfirmed = true;
    }

    handleConfirmEdit(){
        this.editRow = false;
    }

    selectedExercise: ExerciseVM<SetWeightMetrics> | ExerciseVM<SetTimeMetrics> | ExerciseVM<SetMetrics> | null = null;
    handleSelectExercise(exercise: ExerciseVM<SetMetrics>){
        this.selectedExercise = exercise;
    }

    handleToggleComplete(exerciseSet: ExerciseSetVM<SetMetrics>){
        exerciseSet.completed = !exerciseSet.completed;
    }
    handleEditRow(exerciseSet: ExerciseSetVM<SetMetrics>){
        this.selectedExercise?.sets.forEach(x => x.isEditing = false);
        exerciseSet.isEditing = true;
    }

    stopWatchStartTime: DateTime | null = null;
    pausedStopWatchDuration: Duration | null = null;
    stopWatchStopped = false;
    stopWatchIntervalId: any;
    handleStartStopWatch(){
        this.stopWatchStartTime = DateTime.now();
        this.startStopWatch();
    }
    handleStopStopWatch(){
        clearInterval(this.stopWatchIntervalId);
        this.stopWatchStopped = true;
        this.pausedStopWatchDuration = this.stopWatch;
    }
    handleResumeStopWatch(){
        this.stopWatchStartTime = DateTime.now();
        this.startStopWatch();
        this.stopWatchStopped = false;
    }
    handleResetStopWatch(){
        clearInterval(this.stopWatchIntervalId);
        this.stopWatch = null;
        this.pausedStopWatchDuration = null;
        this.stopWatchStopped = false;
    }
    startStopWatch(){
        this.stopWatchIntervalId = setInterval(()=> {
            if(this.pausedStopWatchDuration){
                this.stopWatch = DateTime.now().plus(this.pausedStopWatchDuration).diff(this.stopWatchStartTime!, ['hours', 'minutes', 'seconds']);
                return;
            }

            this.stopWatch = DateTime.now().diff(this.stopWatchStartTime!, ['hours', 'minutes', 'seconds']);
        }, 50);
    } 
}
