import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-audio-off-svg',
  templateUrl: './audio-off-svg.component.html',
  styleUrls: ['./audio-off-svg.component.scss']
})
export class AudioOffSvgComponent {
  @Input() class = '';
  @Input() style = '';

}
