import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-audio-loop-one',
  templateUrl: './audio-loop-one.component.html',
  styleUrls: ['./audio-loop-one.component.scss']
})
export class AudioLoopOneComponent {
  @Input() class = '';
  @Input() style = '';
}
