import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-audio-loop',
  templateUrl: './audio-loop.component.html',
  styleUrls: ['./audio-loop.component.scss']
})
export class AudioLoopComponent {
  @Input() class = '';
  @Input() style = '';
}
