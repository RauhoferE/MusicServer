import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-audio-svg',
  templateUrl: './audio-svg.component.html',
  styleUrls: ['./audio-svg.component.scss']
})
export class AudioSvgComponent {
  @Input() class = '';
  @Input() style = '';

}
