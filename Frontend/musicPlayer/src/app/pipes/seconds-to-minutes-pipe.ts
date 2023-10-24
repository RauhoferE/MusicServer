import { Pipe, PipeTransform } from "@angular/core";

@Pipe({name: 'secondsToMinute'})
export class SecondsToMinutePipe implements PipeTransform {
    transform(value: number, ...args: any[]) {
        let minutes = Math.floor(value / 60);
        let extraSeconds = Math.floor(value % 60);
        let minuteString = minutes.toString();
        let extraSecondsString = extraSeconds.toString();

        if (minutes < 10) {
            minuteString = '0' + minutes;
        }

        if (extraSeconds < 10) {
            extraSecondsString = '0' + extraSeconds;
        }

        return `${minuteString}:${extraSecondsString}`;
    }
    
}