import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { environment } from '../../environments/environment';

interface ChatMessage {
  text: string;
  author: 'user' | 'bot';
}

interface ChatResponse {
  sessionId: string | null;
  reply: string;
}

@Component({
  standalone: true,
  selector: 'student-chat',
  imports: [CommonModule, FormsModule],
  templateUrl: './student-chat.component.html',
  styleUrls: ['./student-chat.component.scss']
})
export class StudentChatComponent {
  protected readonly messages = signal<ChatMessage[]>([]);
  protected readonly input = signal('');
  protected readonly sessionId = signal<string | null>(null);
  protected readonly status = signal('Start a new session or continue the chat.');
  protected readonly loading = signal(false);
  private abortController: AbortController | null = null;
  private requestStopped = false;

  constructor() {}

  protected async sendMessage(): Promise<void> {
    if (this.loading()) {
      return;
    }

    const text = this.input().trim();
    if (!text) {
      this.status.set('Please type a message before sending.');
      return;
    }

    const payload = {
      message: text,
      sessionId: this.sessionId()
    };

    this.messages.update((current) => [...current, { text, author: 'user' }]);
    this.messages.update((current) => [...current, { text: '...', author: 'bot' }]);
    const typingIndex = this.messages().length - 1;
    this.input.set('');
    this.loading.set(true);
    this.status.set('Sending your message to the student agent...');
    this.scrollToBottom();

    this.abortController = new AbortController();
    this.requestStopped = false;

    try {
      const response = await fetch(`${environment.apiBaseUrl}/Student/StudentsAgent`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload),
        signal: this.abortController.signal
      });

      if (!response.ok) {
        throw new Error(`Server returned ${response.status}`);
      }

      const data = (await response.json()) as ChatResponse;

      if (data?.sessionId) {
        this.sessionId.set(data.sessionId);
      }

      const reply = data?.reply ?? 'No reply received from the student agent.';
      await this.animateBotReply(typingIndex, reply);
      this.status.set('Student agent replied successfully.');
    } catch (error) {
      if (error instanceof DOMException && error.name === 'AbortError') {
        this.updateMessageAt(typingIndex, 'Request stopped by the user.');
        this.status.set('Request was cancelled.');
      } else {
        const message = error instanceof Error ? error.message : 'Unable to reach the student agent.';
        this.updateMessageAt(typingIndex, message);
        this.status.set('There was an error sending your request.');
      }
    } finally {
      this.loading.set(false);
      this.abortController = null;
      this.requestStopped = false;
    }
  }

  protected onActionClick(): void {
    if (this.loading()) {
      this.cancelRequest();
      return;
    }

    this.sendMessage();
  }

  protected cancelRequest(): void {
    if (!this.abortController) {
      this.requestStopped = true;
      return;
    }

    this.requestStopped = true;
    this.abortController.abort();
    this.abortController = null;
    this.loading.set(false);
    this.status.set('Request has been stopped.');
  }

  private async animateBotReply(index: number, reply: string): Promise<void> {
    const tokens = reply.split(/(\s+)/);
    let current = '';

    for (const token of tokens) {
      if (this.requestStopped) {
        this.updateMessageAt(index, 'Response generation stopped.');
        return;
      }

      current += token;
      this.updateMessageAt(index, current);
      this.scrollToBottom();
      const delay = token.trim() ? 40 : 20;
      await this.delay(delay);
    }
  }

  private updateMessageAt(index: number, text: string): void {
    this.messages.update((current) =>
      current.map((message, i) =>
        i === index
          ? {
              ...message,
              text
            }
          : message
      )
    );
  }

  private delay(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const chatWindow = document.querySelector('.chat-window');
      if (chatWindow) {
        chatWindow.scrollTop = chatWindow.scrollHeight;
      }
    });
  }

  protected startNewChat(): void {
    this.sessionId.set(null);
    this.messages.set([]);
    this.status.set('New chat started. Send a message to the student agent.');
  }
}
