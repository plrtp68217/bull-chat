import { ref, type Ref } from 'vue';
import type { ObserverConfig } from './interfaces';
import { chatScrollConfig } from './config';

export default function useObserver(
  triggerElement: string,
  callbacks: {
    onVisible: () => void;
    onHidden?: () => void;
  },
  config: ObserverConfig = chatScrollConfig,
) {
  const observer: Ref<IntersectionObserver | null> = ref(null);
  const targetElement: Ref<Element | null> = ref(null);

  const initObserver = () => {
    const defaultConfig: ObserverConfig = {
      root: null,
      rootMargin: '0px',
      threshold: 0.1,
      ...config
    };

    observer.value = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          callbacks.onVisible();
        } else if (callbacks.onHidden) {
          callbacks.onHidden();
        }
      });
    }, defaultConfig);

    targetElement.value = document.querySelector(triggerElement);
    
    if (targetElement.value) {
      observer.value.observe(targetElement.value);
    } else {
      console.warn(`Element "${triggerElement}" not found`);
    }
  };

  const disconnectObserver = () => {
    if (observer.value) {
      observer.value.disconnect();
    }
  };

  return {
    triggerElement,
    initObserver,
    disconnectObserver
  };
}